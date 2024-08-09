using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Vinted.Hw.Persistence
{
    public abstract class BaseRepository<T>
    {
        private readonly string _filePath;

        protected BaseRepository(string filePath)
        {
            _filePath = filePath;
        }

        protected List<T> GetEntities()
        {
            if (!File.Exists(_filePath))
            {
                return new List<T>();
            }

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }

            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        protected T GetEntity()
        {
            if (!File.Exists(_filePath))
            {
                return default;
            }

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json);
        }

        protected void SaveEntities(List<T> entities)
        {
            string json = JsonSerializer.Serialize(entities, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
