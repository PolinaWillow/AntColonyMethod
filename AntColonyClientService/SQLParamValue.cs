using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Service
{
    public class SQLParamValue : IParamValuesRepository
    {
        private readonly AppDbContext _context;

        public SQLParamValue(AppDbContext context) {
            //Зависимость БД
            this._context = context;
        }
    }
}
