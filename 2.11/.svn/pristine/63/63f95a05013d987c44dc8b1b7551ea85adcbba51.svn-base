using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Entity;

namespace Model.DataAccess
{
    public interface IScenarioListAccess
    {
        Task<IList<ScenarioData>> GetScenariosLong(string salesOrgId, DateTime start, DateTime end);

        Task<string> DeleteScenarios(IList<string> scenarioIds);
        Task<string> CreateWorkingScenario(IList<string> scenarioIds);

        Task<string> SaveActiveBudgets(Dictionary<int, bool> scenarioIds, string salesOrg, DateTime? startDate, DateTime? endDate, object obj);

        Task<string> CloseScenarios(IList<string> scenarioIds);
        Task<string> CopyScenarios(IList<string> scenarioIds);

        Task<string> LastClosedScenarios(DateTime date);
        string FilterStatusProc { get; }
        string SaveDefaultsProc { get; }
        string FilterDatesProc { get; }
        string GetScenariosProc();
        string GetLastDateProc();
    }
}