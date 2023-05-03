using AntColonyClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Service
{
    public interface IParamValuesRepository
    {
        Task<IEnumerable<ParamElems>> GetAllParamValues(int idParam);

        Task<ParamElems> GetParamValueById(int id);

        Task<ParamElems> UpdateParamValue(ParamElems updateParamValue);

        Task<ParamElems> AddParamValue(ParamElems newParamValue);

        Task<int> DeleteParamValue(int id);

        Task<int> GetValueCount(int idParam);
    }
}
