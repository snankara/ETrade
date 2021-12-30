using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly ITokenHelper _tokenHelper;

        public AuthManager(IUserService userService, ICustomerService customerService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _customerService = customerService;
            _tokenHelper = tokenHelper;
        }

        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            var claims = _userService.GetClaims(user);
            var accessToken = _tokenHelper.CreateToken(user, claims);
            return new SuccessDataResult<AccessToken>(accessToken, "Access Token Created !");
        }

        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            var user = _userService.GetByMail(userForLoginDto.Email);
            if (user.Data == null)
            {
                return new ErrorDataResult<User>("User Not Found !");
            }

            if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password,user.Data.PasswordHash, user.Data.PasswordSalt))
            {
                return new ErrorDataResult<User>("Password Error !");
            }

            return new SuccessDataResult<User>(user.Data, "Successful Login !");
        }

        public IDataResult<User> Register(UserForRegisterDto userForRegisterDto)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePassswordHash(userForRegisterDto.Password, out passwordHash, out passwordSalt);

            var user = new User
            {
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                Email = userForRegisterDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };

            _userService.Add(user);

            var customer = new Customer
            {
                UserId = user.Id
            };

            _customerService.Add(customer);

            return new SuccessDataResult<User>(user, "Successful Register !");
        }

        public IResult UserExists(string email)
        {
            var user = _userService.GetByMail(email);

            if (user.Data == null)
            {
                return new SuccessResult();
            }

            return new ErrorResult("User Already Exists");
        }
    }
}
