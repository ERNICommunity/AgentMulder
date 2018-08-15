﻿using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;

namespace AgentMulder.ReSharper.Plugin.Components
{
    public interface IPatternManager
    {
        IEnumerable<RegistrationInfo> GetRegistrationsForFile(IPsiSourceFile sourceFile);
        IEnumerable<RegistrationInfo> GetAllRegistrations();
        void Refresh();

        event EventHandler Save;
    }
}