using System.Data.Entity;
using Microsoft.Data.SqlClient;

namespace AcmeBankApp.Data
{
    public class SqlClientDbConfiguration : DbConfiguration
    {
        public SqlClientDbConfiguration()
        {
            SetProviderServices("Microsoft.Data.SqlClient", Microsoft.Data.SqlClient.EntityFramework.SqlClientProviderServices.Instance);
            SetProviderFactory("Microsoft.Data.SqlClient", Microsoft.Data.SqlClient.SqlClientFactory.Instance);
        }
    }
}
