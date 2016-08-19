namespace Exceedra.CellsGrid
{
    public interface ICanvasErrorReportable
    {
        event CanvasErrorHandler ErrorReported;
    }

    public delegate void CanvasErrorHandler(string errorMessage);
}