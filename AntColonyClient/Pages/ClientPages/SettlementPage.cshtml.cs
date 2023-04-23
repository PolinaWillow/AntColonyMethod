using AntColonyClient.Models;
using AntColonyClient.Service;
using AntColonyExtLib.DataModel;
using AntColonyExtLib.DataModel.Numerators;
using AntColonyExtLib.Processing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyClient.Pages.ClientPages
{
    public class SettlementPageModel : PageModel
    {
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly ITaskParamsRepository _taskParamRepository;
        private readonly IParamValuesRepository _valueParamRepository;

        public SettlementPageModel(IUserTaskRepository userTaskRepository, ITaskParamsRepository taskParamsRepository, IParamValuesRepository valueParamRepository) {
            //Внедрение зависимостей интерфейса
            _userTaskRepository = userTaskRepository;
            _taskParamRepository = taskParamsRepository;
            _valueParamRepository = valueParamRepository;
        }

        //Получаемые данные из БД
        public UserTask userTask { get; set; }
        public IEnumerable<TaskParams> taskParams { get; set; }
        public IEnumerable<ParamElems> paramValues { get; set; }

        //Входная структура для расчетного блока
        public InputData inputData { get; set; } = new InputData();

        public IActionResult OnGet(int id)
        {
            //Получение входных данных и формирование структуры хранения параметров
            userTask = _userTaskRepository.GetTaskById(id);
            if (userTask == null) { return RedirectToPage("/NotFound"); }
            else {
                taskParams = _taskParamRepository.GetAllTaskParams(id);
                if (taskParams != null) {
                    //Заполнение списка параметров
                    foreach (var param in taskParams) {
                        paramValues = _valueParamRepository.GetAllParamValues(param.Id);
                        if (paramValues != null) {
                            Param newParam = new Param();
                            newParam.defParam.numParam = param.NumParam;
                            newParam.defParam.typeParam = param.TypeParam;
                            newParam.defParam.valuesCount = _valueParamRepository.GetValueCount(param.Id);
                            //Заполнение значений параметра
                            foreach (var val in paramValues) {
                                ParamValue newValue = new ParamValue();
                                newValue.idValue = val.Id;
                                newValue.valueParam = val.ValueParam;
                                newParam.valuesParam.Add(newValue);
                            }
                            for (int i = 0; i < newParam.valuesParam.Count; i++) {
                                newParam.valuesParam[i].numValue = i;
                            }
                            inputData.inputParams.Params.Add(newParam);                          
                        }
                    }
                    //Подсчет числа итераций
                    int count = 1;
                    foreach (var elem in inputData.inputParams.Params) {
                        count = count * elem.valuesParam.Count();
                    }
                    inputData.inputParams.countCombinationsV = count;
                    inputData.antCount = 5;
                    inputData.iterationCount = (long)Math.Ceiling((double)count/inputData.antCount);
                }
                return Page();
            }
        }

        //Разобраться что там с редиректами
        //Добавить функцию запрета нажатия на вкладки до момента нажатия кнопки остановить
        [BindProperty]
        public int taskIsd { get; set; }
        public async void OnPostStartSettlement()
        {
            //Получение входных данных и формирование структуры хранения параметров
            userTask = _userTaskRepository.GetTaskById(taskIsd);
            if (userTask != null)
            {
                taskParams = _taskParamRepository.GetAllTaskParams(taskIsd);
                if (taskParams != null)
                {
                    //Заполнение списка параметров
                    foreach (var param in taskParams)
                    {
                        paramValues = _valueParamRepository.GetAllParamValues(param.Id);
                        if (paramValues != null)
                        {
                            Param newParam = new Param();
                            newParam.defParam.numParam = param.NumParam;
                            newParam.defParam.typeParam = param.TypeParam;
                            newParam.defParam.valuesCount = _valueParamRepository.GetValueCount(param.Id);
                            //Заполнение значений параметра
                            foreach (var val in paramValues)
                            {
                                ParamValue newValue = new ParamValue();
                                newValue.idValue = val.Id;
                                newValue.valueParam = val.ValueParam;
                                newParam.valuesParam.Add(newValue);
                            }
                            for (int i = 0; i < newParam.valuesParam.Count; i++)
                            {
                                newParam.valuesParam[i].numValue = i;
                            }
                            inputData.inputParams.Params.Add(newParam);
                        }
                    }
                    //Подсчет числа итераций
                    int count = 1;
                    foreach (var elem in inputData.inputParams.Params)
                    {
                        count = count * elem.valuesParam.Count();
                    }
                    inputData.inputParams.countCombinationsV = count;
                    inputData.antCount = 5;
                    inputData.iterationCount = (long)Math.Ceiling((double)count / inputData.antCount);
                }
            }

                await SenttlementBlock.Senttlement(inputData);
            
        }
    }
}
