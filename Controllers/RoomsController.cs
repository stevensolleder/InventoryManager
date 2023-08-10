using InventoryManager.DataTransferObjects;
using InventoryManager.Model;
using InventoryManager.Services;

namespace InventoryManager.Controllers;

public class RoomsController : UniqueRessourceCRUDController<Room, CreateRoom, UpdateRoom>
{
    public RoomsController(DatabaseContext databaseContext) : base(databaseContext,createUpdateRoom =>  new Room(createUpdateRoom.Name)){}
};