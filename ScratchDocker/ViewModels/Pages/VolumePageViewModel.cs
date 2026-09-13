using System.Threading.Tasks;
using ScratchDocker.Models;

namespace ScratchDocker.ViewModels.Pages;

public class VolumePageViewModel : ViewModelBase, IPageViewModel
{
    public Task LoadData()
    {
        return Task.CompletedTask;
    }
}