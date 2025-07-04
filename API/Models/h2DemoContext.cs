﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class h2DemoContext : DbContext
{
    public h2DemoContext(DbContextOptions<h2DemoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingRoom> BookingRooms { get; set; }

    public virtual DbSet<BookingUser> BookingUsers { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingRoom>(entity =>
        {
            entity.HasIndex(e => e.BookingId, "IX_BookingRooms_BookingId");

            entity.HasIndex(e => e.RoomId, "IX_BookingRooms_RoomId");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingRooms).HasForeignKey(d => d.BookingId);

            entity.HasOne(d => d.Room).WithMany(p => p.BookingRooms).HasForeignKey(d => d.RoomId);
        });

        modelBuilder.Entity<BookingUser>(entity =>
        {
            entity.HasIndex(e => e.BookingId, "IX_BookingUsers_BookingId");

            entity.HasIndex(e => e.UserId, "IX_BookingUsers_UserId");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingUsers).HasForeignKey(d => d.BookingId);

            entity.HasOne(d => d.User).WithMany(p => p.BookingUsers).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}