using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamerasInfo.Context
{
    public class CamInfoContext: DbContext
    {
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<Config> Configs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseOracle("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)" +
            //    "(HOST=RCODAX8-SCAN)(PORT=1521))(CONNECT_DATA=(SERVER = DEDICATED)" +
            //    "(SERVICE_NAME=RENOVIAS.renoviasconcessionaria.local)));;" +
            //    "User Id=CAMERASINFO;Password=#1oJH7#3+Ld4_gqZgP1;", b =>
            //b.UseOracleSQLCompatibility("11"));
            optionsBuilder.UseOracle("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)" +
                "(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVER = DEDICATED)" +
                "(SERVICE_NAME=XEPDB1)));;" +
                "User Id=TESTE;Password=teste;", b =>
            b.UseOracleSQLCompatibility("11"));
        }
    }
}
