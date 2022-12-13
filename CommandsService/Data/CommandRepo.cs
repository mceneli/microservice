using CommandsService.Models;

namespace CommandsService.Data{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRepo(AppDbContext context)
		{
			_context = context;
		}
        void ICommandRepo.CreateCommand(int platformId, Command command)
        {
            if(command == null){
				throw new ArgumentNullException(nameof(command));
			}
			command.PlatformId = platformId;
			_context.Commands.Add(command);
        }

        void ICommandRepo.CreatePlatform(Platform plat)
        {
            if(plat == null){
				throw new ArgumentNullException(nameof(plat));
			}
			_context.Platforms.Add(plat);
        }

        IEnumerable<Platform> ICommandRepo.GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        Command ICommandRepo.GetCommand(int platformId, int commandId)
        {
            return _context.Commands
				.Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefault();
        }

        IEnumerable<Command> ICommandRepo.GetCommandsForPlatform(int platformId)
        {
            return _context.Commands
				.Where(c => c.PlatformId == platformId)
				.OrderBy(c => c.Platform.Name);
        }

        bool ICommandRepo.PlatformExists(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);
        }

        bool ICommandRepo.SaveChanges()
        {
            return(_context.SaveChanges() >= 0);
        }
    }
}