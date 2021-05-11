using API.Entities;

namespace API.Interfaces
{
    public interface ITockenService
    {
        string CreateTocken(AppUser user);
    }
}