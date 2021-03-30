using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwetterBook.Contracts;
using TwetterBook.Contracts.Requests;
using TwetterBook.Contracts.V1.Requests;
using TwetterBook.Contracts.V1.Responses;
using TwetterBook.Services;

namespace TwetterBook.Controllers.V1
{
    public class IdentityController : Controller
    {

        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

     
        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody]UserRegistrationRequest request)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                   Errors = ModelState.Values.SelectMany(x=>x.Errors.Select(xx=>xx.ErrorMessage))
                });
            }
            var Auth = await _identityService.CreateAsync(request.Email, request.PassWord);


            if (!Auth.Success)
            {
                return BadRequest( new AuthFailedResponse
                {
                    Errors = Auth.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = Auth.Token
            });

        }


        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody]UserLoginRequest request)
        {
            var Auth = await _identityService.LoginAsync(request.Email, request.PassWord);

            if (!Auth.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = Auth.Errors
                });
            }


            return Ok(new AuthSuccessResponse
            {
                Token = Auth.Token
            });


        }


    }
}
