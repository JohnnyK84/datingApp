using System;
using System.Threading.Tasks;
using datingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace datingApp.API.Data
{
    //This Repository is responsible for querying database via Entity Framework
    public class AuthoRepository : IAuthRepository //Inherit from Interface
    {
        //Inject DataContext in this class
        private readonly DataContext _context; //initiate private readonly Datacontext to Repository
        public AuthoRepository(DataContext context) //set DataContext _context
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password) //Login Method
        {
            //use FirstOrDefault method to check if user exists => return user if exists else returns null
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null) 
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) //call method to return true or false if password matches 
                return null;
            
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) 
            //anything in block below will be disposed of after it has been used due to using statement
            {
                //compute hash for password passing in hash for passwordSalt
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                //check if password hashes match and return true or false 
                for (int i=0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }

        public async Task<User> Register(User user, string password) //Register new user and pass take in user input
        {
            byte[] passwordHash, passwordSalt;

            /*call method to generate Password Hash using out to pass a reference to update the 
            variables byte[] passwordHash, passwordSalt */
            CreatePasswordHash(password, out passwordHash, out passwordSalt); 

            user.PasswordHash = passwordHash; //set user passwordHash
            user.PasswordSalt = passwordSalt; //set user passwordSalt

            //update server with user details
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        //method to create Password Hash
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) //method to hash password
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) // one of many ways to Hash a password
            //anything in block below will be disposed of after it has been used due to using statement
            {
                // Setting passwordSalt and passwordHash
                passwordSalt = hmac.Key; //Generate Randomly computed Salt Key
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); //pass in password as byte[] array
            }
        }

        //method to check if User exists
        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))
                return true;

            return false;
        }
    }
}