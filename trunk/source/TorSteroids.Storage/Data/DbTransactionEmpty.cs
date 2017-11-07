using System.Data;
using System.Data.Common;

namespace TorSteroids.Storage.Data
{
    internal class DbTransactionEmpty : DbTransaction
    {
        private readonly DbConnection _connection;
        
        internal DbTransactionEmpty(DbConnection connection)
        {
            _connection = connection;
        }

        public override void Commit(){}
        public override void Rollback(){}
        protected override DbConnection DbConnection { get { return _connection; } }
        public override IsolationLevel IsolationLevel { get{ return IsolationLevel.Unspecified;} }
        
    }

}
