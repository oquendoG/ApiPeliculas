﻿using ApiPeliculas.Shared;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(){}

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Category> Categories { get; set; }
}