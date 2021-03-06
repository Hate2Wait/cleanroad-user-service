﻿using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Gareon.WebService.Cqrs.Abstractions.Bus;
using Gareon.WebService.Cqrs.Abstractions.Commands;
using Gareon.WebService.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gareon.WebService.Controllers.V1
{
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {
        private readonly ICommandBus bus;
        private readonly IMapper mapper;

        public AuthController(ICommandBus bus, IMapper mapper)
        {
            this.bus = bus;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public async Task<IActionResult> Register(CancellationToken cancellationToken, [FromBody] TbUserRegisterDto registerDto)
        {
            var registerCommand = this.mapper.Map<TbUserRegisterCommand>(registerDto);
            
            await this.bus.SendAsync(registerCommand, cancellationToken);

            return this.NoContent();
        }
    }
}