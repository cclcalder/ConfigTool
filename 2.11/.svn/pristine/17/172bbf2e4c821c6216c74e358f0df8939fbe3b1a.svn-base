using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Model.Entity.Canvas
{
    public interface ICanvasAccessor
    {
        Task<IList<Insight>> GetInsights();
        Task<XElement> GetFilters(string insightId);
        Task<string> SaveDefaults(string insightId, XElement filters);
    }
}
