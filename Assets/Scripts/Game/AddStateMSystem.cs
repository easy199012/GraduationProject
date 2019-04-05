﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitas;
using MyService;

public class AddStateMSystem : IInitializeSystem
{
    PlayerContext PlayerContext;
    GameContext GameContext;
    PlayerEntity PlayerEntity;
    GameEntity GameEntity;

    public AddStateMSystem(Contexts contexts)
    {
        PlayerContext = contexts.player;
        GameContext = contexts.game;
    }

    public void Initialize()
    {
        PlayerEntity = PlayerContext.localPlayerEntity;
        GameEntity = GameContext.mainCameraEntity;
        GameEntity.AddViewModeController(new MyService.CameraViewModeController(GameEntity, PlayerEntity));
        PlayerEntity.AddViewMode(new PlayerViewModeController(ref PlayerEntity, GameEntity));
    }
}

