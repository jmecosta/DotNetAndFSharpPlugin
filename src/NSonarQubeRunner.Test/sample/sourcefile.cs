﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SonarQubeViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The changed event handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VSSonarExtensionUi.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    ///     The changed event handler.
    /// </summary>
    /// <param name="sender">
    ///     The sender.
    /// </param>
    /// <param name="e">
    ///     The e.
    /// </param>
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    ///     The sonar qube view viewModel.
    /// </summary>
    public partial class SonarQubeViewModel
    {
        public int xpro = 0;
    }
}