using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface ITockenService
    {
        Task<string> CreateTocken(AppUser user);
    }
}