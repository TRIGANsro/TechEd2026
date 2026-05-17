using WPF2DBinding.Models;

namespace WPF2DBinding.Services;

public interface IDataService
{
    Task SaveZajimavostiAsync(string imageFileName, IEnumerable<Zajimavost> zajimavosti);
    Task<List<Zajimavost>> LoadZajimavostiAsync(string imageFileName);
}