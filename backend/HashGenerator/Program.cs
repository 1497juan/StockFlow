using Microsoft.AspNetCore.Identity;
using StockFlow.Domain.Entities;

var user = new User
{
    Id = 1,
    Username = "administrador"
};

var hasher = new PasswordHasher<User>();
var hash = hasher.HashPassword(user, "123prueba");

Console.WriteLine(hash);