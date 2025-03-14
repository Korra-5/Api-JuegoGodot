using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using JuegoApi.Models;

namespace JuegoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JuegoItemsController : ControllerBase
    {
        // Declaramos la colección de MongoDB donde se guardarán los registros
        private readonly IMongoCollection<JuegoItem> _timerCollection;

        // Cadena de conexión a MongoDB
        private readonly string _connectionString = "mongodb+srv://dani05corral:JNqUILsjfRSsHh7w@cluster0.iukey.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";

        /// <summary>
        /// Constructor del controller. Inicializa el cliente de MongoDB, la base y la colección.
        /// </summary>
        public JuegoItemsController()
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase("JuegoGodot");
            _timerCollection = database.GetCollection<JuegoItem>("Timer");
        }

        // GET: api/JuegoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JuegoItem>>> GetJuegoItems()
        {
            var juegoItems = await _timerCollection.Find(item => true).ToListAsync();
            return Ok(juegoItems);
        }

        // NUEVO ENDPOINT: GET: api/JuegoItems/top5
        [HttpGet("top5")]
        public async Task<ActionResult<IEnumerable<JuegoItem>>> GetTop5JuegoItems()
        {
            // Obtener los 5 mejores tiempos (menor tiempo = mejor puntuación)
            var top5Items = await _timerCollection
                .Find(item => true)
                .Sort(Builders<JuegoItem>.Sort.Ascending(item => item.Time))
                .Limit(5)
                .ToListAsync();

            return Ok(top5Items);
        }

        // GET: api/JuegoItems/{name}
        [HttpGet("{name}")]
        public async Task<ActionResult<JuegoItem>> GetJuegoItem(string name)
        {
            var juegoItem = await _timerCollection.Find(item => item.Name == name).FirstOrDefaultAsync();
            if (juegoItem == null)
            {
                return NotFound();
            }
            return Ok(juegoItem);
        }

        // PUT: api/JuegoItems/{name}
        [HttpPut("{name}")]
        public async Task<IActionResult> PutJuegoItem(string name, JuegoItem juegoItem)
        {
            if (name != juegoItem.Name)
            {
                return BadRequest();
            }

            var replaceResult = await _timerCollection.ReplaceOneAsync(item => item.Name == name, juegoItem);

            if (replaceResult.MatchedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
// POST: api/JuegoItems
[HttpPost]
public async Task<ActionResult<JuegoItem>> PostJuegoItem(JuegoItem juegoItem)
{
    // Formatear el tiempo a 2 decimales para mantener consistencia
    juegoItem.Time = Math.Round(juegoItem.Time, 2);
    
    // Se busca si ya existe un registro para este jugador
    var existingItem = await _timerCollection.Find(item => item.Name == juegoItem.Name).FirstOrDefaultAsync();

    if (existingItem != null)
    {
        // Redondear también el tiempo existente para comparar con la misma precisión
        double existingTime = Math.Round(existingItem.Time, 2);
        
        // Actualiza el tiempo solo si el nuevo es mejor (menor) que el almacenado
        if (juegoItem.Time < existingTime)
        {
            existingItem.Time = juegoItem.Time;
            await _timerCollection.ReplaceOneAsync(item => item.Name == juegoItem.Name, existingItem);
        }
        return CreatedAtAction(nameof(GetJuegoItem), new { name = juegoItem.Name }, existingItem);
    }
    else
    {
        // Si es un nuevo registro, se inserta en la colección.
        await _timerCollection.InsertOneAsync(juegoItem);
        return CreatedAtAction(nameof(GetJuegoItem), new { name = juegoItem.Name }, juegoItem);
    }
}

        // DELETE: api/JuegoItems/{name}
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteJuegoItem(string name)
        {
            var deleteResult = await _timerCollection.DeleteOneAsync(item => item.Name == name);
            if (deleteResult.DeletedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}