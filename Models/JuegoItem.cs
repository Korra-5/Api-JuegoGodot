using System.ComponentModel.DataAnnotations;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace JuegoApi.Models;

public class JuegoItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]
    public string? Name { get; set; }

    [BsonElement("Time")]
    public double Time { get; set; }
}