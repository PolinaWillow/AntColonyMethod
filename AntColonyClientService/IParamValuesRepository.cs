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
        IEnumerable<ParamElems> GetAllParamValues(int idParam);

        ParamElems GetParamValueById(int id);

        ParamElems UpdateParamValue(ParamElems updateParamValue);

        ParamElems AddParamValue(ParamElems newParamValue);

        int DeleteParamValue(int id);

        int GetValueCount(int idParam);
    }
}
