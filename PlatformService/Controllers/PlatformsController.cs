using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using PlatformService.AsyncDataServices;
using Microsoft.AspNetCore.Authorization;

namespace PlatformService.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase{
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
		private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository,
									IMapper mapper,
									ICommandDataClient commandDataClient,
									IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
			_messageBusClient = messageBusClient;
        }

        [HttpGet, Authorize(Roles = "User")]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms(){
            Console.WriteLine("-> Getting Platforms...");

            var platformItem = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id){
            Console.WriteLine("-> Getting Platform By Id...");

            var platformItem = _repository.GetPlatformById(id);

            if(platformItem != null){
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto){
            Console.WriteLine("-> Creating Platform...");

            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);
			
			// Send Sync Message
            try{
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }catch(Exception ex){
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }
			
			// Send Async Message
			try{
				var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
				platformPublishedDto.Event = "Platform_Published";
				_messageBusClient.PublishNewPlatform(platformPublishedDto);
			}
			catch(Exception ex){
				Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
			}

            return CreatedAtRoute(nameof(GetPlatformById), new { platformReadDto.Id },  platformReadDto);
        }

    }
}