-- Legacy Database Setup Script for AcmeBankApp
-- Typical SQL Server LocalDB setup for 2018-2019 era

USE master
GO

-- Drop database if exists (for development)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'AcmeBank')
BEGIN
    ALTER DATABASE AcmeBank SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE AcmeBank
END
GO

-- Create database
CREATE DATABASE AcmeBank
ON 
( NAME = 'AcmeBank',
  FILENAME = 'C:\Temp\AcmeBank.mdf',
  SIZE = 100MB,
  MAXSIZE = 1GB,
  FILEGROWTH = 10MB )
LOG ON
( NAME = 'AcmeBank_Log',
  FILENAME = 'C:\Temp\AcmeBank_Log.ldf',
  SIZE = 10MB,
  MAXSIZE = 100MB,
  FILEGROWTH = 1MB )
GO

USE AcmeBank
GO

-- Legacy: Create Users table
CREATE TABLE [dbo].[Users] (
    [UserId] INT IDENTITY(1,1) NOT NULL,
    [UserName] NVARCHAR(50) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL, -- Legacy: Should be properly hashed
    [FirstName] NVARCHAR(100) NULL,
    [LastName] NVARCHAR(100) NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [LastLogin] DATETIME2 NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [SecurityQuestion] NVARCHAR(255) NULL, -- Legacy: Security issue
    [SecurityAnswer] NVARCHAR(255) NULL,   -- Legacy: Should be hashed
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [UQ_Users_UserName] UNIQUE ([UserName]),
    CONSTRAINT [UQ_Users_Email] UNIQUE ([Email])
)
GO

-- Legacy: Create Accounts table
CREATE TABLE [dbo].[Accounts] (
    [AccountId] INT IDENTITY(1,1) NOT NULL,
    [AccountNumber] NVARCHAR(20) NOT NULL,
    [UserId] INT NOT NULL,
    [AccountType] NVARCHAR(50) NOT NULL, -- Checking, Savings, Credit
    [Balance] DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    [AccountName] NVARCHAR(100) NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([AccountId] ASC),
    CONSTRAINT [UQ_Accounts_AccountNumber] UNIQUE ([AccountNumber]),
    CONSTRAINT [FK_Accounts_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId])
)
GO

-- Legacy: Create Transactions table
CREATE TABLE [dbo].[Transactions] (
    [TransactionId] INT IDENTITY(1,1) NOT NULL,
    [FromAccountId] INT NOT NULL,
    [ToAccountId] INT NULL,
    [TransactionType] NVARCHAR(20) NOT NULL, -- Transfer, Deposit, Withdrawal, Payment
    [Amount] DECIMAL(18,2) NOT NULL,
    [Description] NVARCHAR(200) NULL,
    [TransactionDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Completed, Failed
    [InternalNotes] NVARCHAR(500) NULL, -- Legacy: Potential security issue
    CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED ([TransactionId] ASC),
    CONSTRAINT [FK_Transactions_FromAccount] FOREIGN KEY ([FromAccountId]) REFERENCES [Accounts]([AccountId]),
    CONSTRAINT [FK_Transactions_ToAccount] FOREIGN KEY ([ToAccountId]) REFERENCES [Accounts]([AccountId])
)
GO

-- Legacy: Create indexes (not always optimal)
CREATE NONCLUSTERED INDEX [IX_Users_UserName] ON [dbo].[Users] ([UserName])
GO

CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email])
GO

CREATE NONCLUSTERED INDEX [IX_Accounts_UserId] ON [dbo].[Accounts] ([UserId])
GO

CREATE NONCLUSTERED INDEX [IX_Accounts_AccountNumber] ON [dbo].[Accounts] ([AccountNumber])
GO

CREATE NONCLUSTERED INDEX [IX_Transactions_FromAccountId] ON [dbo].[Transactions] ([FromAccountId])
GO

CREATE NONCLUSTERED INDEX [IX_Transactions_ToAccountId] ON [dbo].[Transactions] ([ToAccountId])
GO

CREATE NONCLUSTERED INDEX [IX_Transactions_TransactionDate] ON [dbo].[Transactions] ([TransactionDate])
GO

-- Legacy: Insert sample data
INSERT INTO [dbo].[Users] ([UserName], [Email], [PasswordHash], [FirstName], [LastName], [SecurityQuestion], [SecurityAnswer])
VALUES 
    ('demo', 'demo@acmebank.com', 'password123', 'Demo', 'User', 'What is your favorite color?', 'blue'),
    ('jdoe', 'john.doe@email.com', 'mypassword', 'John', 'Doe', 'What was your first pet?', 'fluffy'),
    ('msmith', 'mary.smith@email.com', 'secret123', 'Mary', 'Smith', 'Where were you born?', 'chicago'),
    ('admin', 'admin@acmebank.com', 'admin123', 'System', 'Administrator', 'What is the answer?', '42')
GO

-- Legacy: Insert sample accounts
INSERT INTO [dbo].[Accounts] ([AccountNumber], [UserId], [AccountType], [Balance], [AccountName])
VALUES 
    ('1001234567', 1, 'Checking', 2500.00, 'Demo Checking'),
    ('1001234568', 1, 'Savings', 15000.00, 'Demo Savings'),
    ('2001234567', 2, 'Checking', 1200.50, 'John Primary Checking'),
    ('2001234568', 2, 'Savings', 8500.75, 'John Emergency Fund'),
    ('2001234569', 2, 'Credit', -850.25, 'John Credit Card'),
    ('3001234567', 3, 'Checking', 3200.00, 'Mary Checking'),
    ('3001234568', 3, 'Savings', 25000.00, 'Mary Retirement Fund'),
    ('4001234567', 4, 'Checking', 100000.00, 'Admin Account')
GO

-- Legacy: Insert sample transactions
INSERT INTO [dbo].[Transactions] ([FromAccountId], [ToAccountId], [TransactionType], [Amount], [Description], [Status], [InternalNotes])
VALUES 
    (1, 2, 'Transfer', 500.00, 'Transfer to savings', 'Completed', 'User-initiated transfer'),
    (3, NULL, 'Deposit', 1000.00, 'Payroll deposit', 'Completed', 'Direct deposit from employer ABC Corp'),
    (4, NULL, 'Withdrawal', 200.00, 'ATM withdrawal', 'Completed', 'ATM location: Main St branch'),
    (6, 7, 'Transfer', 300.00, 'Monthly savings', 'Completed', 'Automatic transfer'),
    (1, NULL, 'Payment', 150.00, 'Electric bill payment', 'Completed', 'Payment to PowerCorp - Account# 123456789'),
    (3, 1, 'Transfer', 75.00, 'Friend repayment', 'Completed', 'Venmo-style transfer'),
    (5, NULL, 'Payment', 50.00, 'Credit card payment', 'Failed', 'Insufficient funds in source account'),
    (2, NULL, 'Deposit', 25.00, 'Interest payment', 'Completed', 'Monthly interest calculation'),
    (8, NULL, 'Deposit', 5000.00, 'Large deposit', 'Completed', 'Cash deposit - requires reporting to IRS'),
    (1, NULL, 'Withdrawal', 40.00, 'Coffee shop', 'Completed', 'Debit card transaction at Starbucks #4521')
GO

-- Legacy: Create views for reporting
CREATE VIEW [dbo].[vw_AccountSummary] AS
SELECT 
    u.UserName,
    u.FirstName + ' ' + u.LastName AS FullName,
    a.AccountNumber,
    a.AccountType,
    a.AccountName,
    a.Balance,
    a.CreatedDate,
    (SELECT COUNT(*) FROM Transactions t WHERE t.FromAccountId = a.AccountId OR t.ToAccountId = a.AccountId) AS TransactionCount
FROM Users u
INNER JOIN Accounts a ON u.UserId = a.UserId
WHERE u.IsActive = 1 AND a.IsActive = 1
GO

-- Legacy: Create stored procedures (with potential SQL injection vulnerabilities)
CREATE PROCEDURE [dbo].[sp_GetUserTransactions]
    @UserName NVARCHAR(50)
AS
BEGIN
    -- Legacy: This procedure has SQL injection vulnerability
    DECLARE @SQL NVARCHAR(1000)
    SET @SQL = 'SELECT t.*, a.AccountNumber, a.AccountType 
                FROM Transactions t 
                INNER JOIN Accounts a ON t.FromAccountId = a.AccountId 
                INNER JOIN Users u ON a.UserId = u.UserId 
                WHERE u.UserName = ''' + @UserName + ''' 
                ORDER BY t.TransactionDate DESC'
    
    EXEC sp_executesql @SQL
END
GO

-- Legacy: Create function for balance calculation
CREATE FUNCTION [dbo].[fn_CalculateAccountBalance](@AccountId INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @Balance DECIMAL(18,2) = 0
    
    -- Get base balance
    SELECT @Balance = Balance FROM Accounts WHERE AccountId = @AccountId
    
    RETURN @Balance
END
GO

-- Legacy: Create trigger for audit trail (but missing proper security)
CREATE TRIGGER [dbo].[tr_Transactions_Audit] 
ON [dbo].[Transactions]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Legacy: Simple audit without proper security considerations
    IF EXISTS (SELECT * FROM inserted)
    BEGIN
        INSERT INTO [dbo].[AuditLog] (TableName, Action, RecordId, ChangedBy, ChangedDate, Details)
        SELECT 'Transactions', 'INSERT/UPDATE', TransactionId, SYSTEM_USER, GETDATE(), 
               'Transaction: ' + CAST(Amount AS NVARCHAR(50)) + ' from Account ' + CAST(FromAccountId AS NVARCHAR(10))
        FROM inserted
    END
    
    IF EXISTS (SELECT * FROM deleted) AND NOT EXISTS (SELECT * FROM inserted)
    BEGIN
        INSERT INTO [dbo].[AuditLog] (TableName, Action, RecordId, ChangedBy, ChangedDate, Details)
        SELECT 'Transactions', 'DELETE', TransactionId, SYSTEM_USER, GETDATE(), 
               'Deleted transaction: ' + CAST(Amount AS NVARCHAR(50))
        FROM deleted
    END
END
GO

-- Legacy: Create audit log table (minimal implementation)
CREATE TABLE [dbo].[AuditLog] (
    [AuditId] INT IDENTITY(1,1) NOT NULL,
    [TableName] NVARCHAR(50) NOT NULL,
    [Action] NVARCHAR(10) NOT NULL,
    [RecordId] INT NOT NULL,
    [ChangedBy] NVARCHAR(100) NOT NULL,
    [ChangedDate] DATETIME2 NOT NULL,
    [Details] NVARCHAR(500) NULL,
    CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([AuditId] ASC)
)
GO

-- Legacy: Grant permissions (overly permissive)
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Users] TO [public]
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Accounts] TO [public]  
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Transactions] TO [public]
GRANT SELECT ON [dbo].[vw_AccountSummary] TO [public]
GRANT EXECUTE ON [dbo].[sp_GetUserTransactions] TO [public]
GO

PRINT 'AcmeBank database created successfully!'
PRINT 'Sample users created:'
PRINT '  demo/password123'
PRINT '  jdoe/mypassword'
PRINT '  msmith/secret123'
PRINT '  admin/admin123'
PRINT ''
PRINT 'Database contains sample accounts and transactions for testing.'
PRINT 'WARNING: This database contains security vulnerabilities for educational purposes.'
GO
