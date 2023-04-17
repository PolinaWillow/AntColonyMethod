using AntColonyClient.Models;
using AntColonyClient.Service.Migrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        public ParamElems AddParamValue(ParamElems newParamValue)
        {
            _context.Database.ExecuteSqlRaw("AddNewParamValue @idParam, @valueParam",
                                             new SqlParameter("@idParam", newParamValue.IdParam),
                                             new SqlParameter("@valueParam", newParamValue.ValueParam));
            return newParamValue;
        }

        public int DeleteParamValue(int id)
        {
            return _context.Database.ExecuteSqlRaw("DeleteParamValue @id", new SqlParameter("id", id));
        }

        public IEnumerable<ParamElems> GetAllParamValues(int idParam)
        {
            return _context.ParamElems.FromSqlRaw<ParamElems>("GetAllParamValues @idParam", new SqlParameter("idParam", idParam)).ToList();
        }

        public ParamElems GetParamValueById(int id)
        {
            return _context.ParamElems.Find(id);
        }

        public int GetValueCount(int idParam)
        {
            return _context.ParamElems.Count(u => u.IdParam == idParam);
            //return _context.ParamElems.FromSqlRaw<ParamElems>("GetValueCount @idParam", new SqlParameter("@idParam", idParam)).Count();
        }

        public ParamElems UpdateParamValue(ParamElems updateParamValue)
        {
            var valueToUpdate = _context.ParamElems.Attach(updateParamValue);
            valueToUpdate.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return updateParamValue;
        }
    }
}
