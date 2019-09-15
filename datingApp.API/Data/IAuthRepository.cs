using System.Threading.Tasks;
using datingApp.API.Models;

namespace datingApp.API.Data
{
    //always start Interface with capital I in c#
    public interface IAuthRepository
    {
        
         Task<User> Register(User user, string password);
         //Method to take input from user to check username and pass details
         Task<User> Login(string username, string password);
        //Method to check if existing username already exists
         Task<bool> UserExists(string username);

    }
}