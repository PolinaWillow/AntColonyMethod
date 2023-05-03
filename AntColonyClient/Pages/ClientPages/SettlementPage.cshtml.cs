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

        public SettlementPageModel(IUserTaskRepository userTaskRepository, ITaskParamsRepository taskParamsRepository, IParamValuesRepository valueParamRepository)
        {
            //��������� ������������ ����������
            _userTaskRepository = userTaskRepository;
            _taskParamRepository = taskParamsRepository;
            _valueParamRepository = valueParamRepository;

            //���������� ������� ������
            cancelTokenSource = new CancellationTokenSource();
            cancelToken = cancelTokenSource.Token;
        }

        //���������� ������ �� ��
        public UserTask userTask { get; set; }
        public IEnumerable<TaskParams> taskParams { get; set; }
        public IEnumerable<ParamElems> paramValues { get; set; }

        //������� ��������� ��� ���������� �����
        public InputData inputData { get; set; } = new InputData();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            //��������� ������� ������ � ������������ ��������� �������� ����������
            userTask = new UserTask();
            userTask = await _userTaskRepository.GetTaskById(id);
            if (userTask == null) { return RedirectToPage("/NotFound"); }
            else
            {
                taskParams = await _taskParamRepository.GetAllTaskParams(id);
                if (taskParams != null)
                {
                    //���������� ������ ����������
                    foreach (var param in taskParams)
                    {
                        paramValues = await _valueParamRepository.GetAllParamValues(param.Id);
                        if (paramValues != null)
                        {
                            Param newParam = new Param();
                            newParam.defParam.numParam = param.NumParam;
                            newParam.defParam.typeParam = param.TypeParam;
                            newParam.defParam.valuesCount = await _valueParamRepository.GetValueCount(param.Id);
                            //���������� �������� ���������
                            foreach (var val in paramValues)
                            {
                                ParamValue newValue = new ParamValue();
                                newValue.idValue = val.Id;
                                newValue.pheromones = 1;
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
                    //������� ����� ��������
                    int count = 1;
                    foreach (var elem in inputData.inputParams.Params)
                    {
                        count = count * elem.valuesParam.Count();
                    }
                    inputData.inputParams.countCombinationsV = count;
                    inputData.antCount = 5;
                    inputData.iterationCount = (long)Math.Ceiling((double)count / inputData.antCount);
                }

                ViewData["view_inputData"] = inputData;
                ViewData["view_taskId"] = id;
                return Page();
            }
        }

        //����������� ��� ��� � �����������
        //�������� ������� ������� ������� �� ������� �� ������� ������� ������ ����������
        [BindProperty]
        public int taskIsd { get; set; }

        //ConsilationToken
        private CancellationTokenSource cancelTokenSource { get; set; }
        public CancellationToken cancelToken { get; set; }

        public async Task<IActionResult> OnPostAsyncStartSettlement(/*CancellationToken cancelToken*/)
        {
            //stopPageReload();
            //��������� ������� ������ � ������������ ��������� �������� ����������
            userTask = await _userTaskRepository.GetTaskById(taskIsd);
            if (userTask != null)
            {
                taskParams = await _taskParamRepository.GetAllTaskParams(taskIsd);
                if (taskParams != null)
                {
                    //���������� ������ ����������
                    foreach (var param in taskParams)
                    {
                        paramValues = await _valueParamRepository.GetAllParamValues(param.Id);
                        if (paramValues != null)
                        {
                            Param newParam = new Param();
                            newParam.defParam.numParam = param.NumParam-1;
                            newParam.defParam.typeParam = param.TypeParam;
                            newParam.defParam.valuesCount = await _valueParamRepository.GetValueCount(param.Id);
                            //���������� �������� ���������
                            foreach (var val in paramValues)
                            {
                                ParamValue newValue = new ParamValue();
                                newValue.idValue = val.Id;
                                newValue.pheromones = 1;
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
                    //������� ����� ��������
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

            //���� ������ ������ ��������
            if (Request.Form.ContainsKey("StopSettlement_btn")) {
                cancelTokenSource.Cancel();
                //return RedirectToPage("/ClientPages/TaskDetails", new { id = taskIsd });
            }
            try
            {
                await SenttlementBlock.Senttlement(inputData, cancelToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("��������� ������");
                return RedirectToPage("/ClientPages/TaskDetails", new { id = taskIsd });
            }

            return RedirectToPage("/ClientPages/TaskDetails", new { id = taskIsd});
        }

    }
}
