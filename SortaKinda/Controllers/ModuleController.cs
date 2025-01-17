﻿using System;
using System.Collections.Generic;
using System.Linq;
using SortaKinda.Interfaces;
using SortaKinda.Models.Enums;
using SortaKinda.System.Modules;

namespace SortaKinda.System;

public class ModuleController : IDisposable
{
    private readonly IEnumerable<IModule> modules;

    public ModuleController()
    {
        modules = new List<IModule>
        {
            new MainInventoryModule(),
            new ArmoryInventoryModule()
        };
    }

    public void Dispose()
    {
        Unload();

        foreach (var module in modules.OfType<IDisposable>())
        {
            module.Dispose();
        }
    }

    public void Load()
    {
        foreach (var module in modules)
        {
            module.LoadModule();
        }
    }

    public void Unload()
    {
        foreach (var module in modules)
        {
            module.UnloadModule();
        }
    }

    public void Update()
    {
        foreach (var module in modules)
        {
            module.UpdateModule();
        }
    }

    public void Sort()
    {
        foreach (var module in modules)
        {
            module.SortModule();
        }
    }

    public void DrawModule(ModuleName module)
    {
        modules.FirstOrDefault(drawableModule => drawableModule.ModuleName == module)?.Draw();
    }
}