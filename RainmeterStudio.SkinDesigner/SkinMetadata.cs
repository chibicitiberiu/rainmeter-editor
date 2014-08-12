
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Model.Generic;

namespace RainmeterStudio.SkinDesignerPlugin
{
    public class SkinMetadata : Property
    {
        /// <summary>
        /// Gets a property indicating the name of the skin
        /// </summary>
        public Property<string> SkinName { get; private set; }

        /// <summary>
        /// Gets a property indicating the author of the skin
        /// </summary>
        public Property<string> Author { get; private set; }

        /// <summary>
        /// Gets a property containing information about this skin (credits, usage instructions, setup etc)
        /// </summary>
        public Property<string> Information { get; private set; }

        /// <summary>
        /// Gets a property indicating the version of this skin
        /// </summary>
        public Property<Version> Version { get; private set; }

        /// <summary>
        /// Gets a property containing licensing information
        /// </summary>
        public Property<string> License { get; private set; }

        /// <summary>
        /// Initializes this metadata property
        /// </summary>
        public SkinMetadata() :
            base("Metadata")
        {
            SkinName = new Property<string>("Name");
            Author = new Property<string>("Author");
            Information = new Property<string>("Information");
            Version = new Property<Version>("Version");
            License = new Property<string>("License");

            Children.Add(SkinName);
            Children.Add(Author);
            Children.Add(Information);
            Children.Add(Version);
            Children.Add(License);
        }
    }
}
