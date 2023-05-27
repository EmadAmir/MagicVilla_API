using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username); // to check if the user is unique
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO); // returns LoginResponse 
        Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
