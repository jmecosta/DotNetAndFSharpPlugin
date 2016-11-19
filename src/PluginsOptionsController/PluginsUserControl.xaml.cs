﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CxxUserControl.xaml.cs" company="Copyright © 2016 jmecsoftware.com">
//     Copyright (C) 2013 [Jorge Costa, jmecosta@gmail.com]
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
// You should have received a copy of the GNU Lesser General Public License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// --------------------------------------------------------------------------------------------------------------------
namespace PluginsOptionsController
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for CxxUserControl.xaml
    /// </summary>
    public partial class PluginsUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsUserControl"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public PluginsUserControl(PluginsOptionsControl controller)
        {
            this.InitializeComponent();
            this.DataContext = controller;
        }
    }
}
