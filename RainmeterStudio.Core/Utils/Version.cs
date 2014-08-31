using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RainmeterStudio.Core.Utils
{
    /// <summary>
    /// Serializable version of the System.Version class.
    /// </summary>
    public class Version : ICloneable, IComparable
    {
        private int _major, _minor, _build, _revision;

        /// <summary>
        /// Gets or sets the major.
        /// </summary>
        /// <value></value>
        [XmlAttribute("major")]
        public int Major
        {
            get
            {
                return _major;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Major out of range.");

                _major = value;
            }
        }

        /// <summary>
        /// Gets or sets the minor.
        /// </summary>
        /// <value></value>
        [XmlAttribute("minor")]
        public int Minor
        {
            get
            {
                return _minor;
            }
            set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException("Minor out of range.");

                _minor = value;
            }
        }

        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        /// <value></value>
        [XmlAttribute("build")]
        public int Build
        {
            get
            {
                return _build;
            }
            set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException("Build out of range.");

                _build = value;
            }
        }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        /// <value></value>
        [XmlAttribute("revision")]
        public int Revision
        {
            get
            {
                return _revision;
            }
            set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException("Revision out of range.");

                _revision = value;
            }
        }

        /// <summary>
        /// Creates a new instance of version.
        /// </summary>
        public Version()
        {
            Major = 1;
            Minor = 0;
            Revision = -1;
            Build = -1;
        }

        /// <summary>
        /// Creates a new instance of Version.
        /// </summary>
        /// <param name="version">Version string</param>
        public Version(string version)
        {
            // Parse
            int major, minor, revision, build;
            Parse(version, out major, out minor, out build, out revision);

            // Commit
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        /// <summary>
        /// Creates a new instance of Version.
        /// </summary>
        /// <param name="major">Major</param>
        public Version(int major)
            : this(major, -1, -1, -1)
        {
        }

        /// <summary>
        /// Creates a new instance of Version.
        /// </summary>
        /// <param name="major">Major</param>
        /// <param name="minor">Minor</param>
        public Version(int major, int minor)
            : this(major, minor, -1, -1)
        {
        }

        /// <summary>
        /// Creates a new instance of Version.
        /// </summary>
        /// <param name="major">Major</param>
        /// <param name="minor">Minor</param>
        /// <param name="build">Build</param>
        public Version(int major, int minor, int build)
            : this(major, minor, build, -1)
        {
        }

        /// <summary>
        /// Creates a new instance of Version.
        /// </summary>
        /// <param name="major">Major</param>
        /// <param name="minor">Minor</param>
        /// <param name="build">Build</param>
        /// <param name="revision">Revision</param>
        public Version(int major, int minor, int build, int revision)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        #region ICloneable Members

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Version(Major, Minor, Build, Revision);
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares to another object.
        /// </summary>
        /// <param name="obj">Other object</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            Version version = obj as Version;

            if (version == null)
            {
                throw new ArgumentException("Argument must be version");
            }

            if (Major != version.Major)
            {
                return (Major - version.Major);
            }
            if (Minor != version.Minor)
            {
                return (Minor - version.Minor);
            }
            if (Build != version.Build)
            {
                return (Build - version.Build);
            }
            if (Revision != version.Revision)
            {
                return (Revision - version.Revision);
            }
            return 0;
        }

        #endregion
        /// <summary>
        /// Equalss the specified obj.
        /// </summary>
        /// <param name="obj">Obj.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Version version = obj as Version;

            if (version == null)
            {
                return false;
            }
            else
            {
                return this.CompareTo(obj) == 0;
            }
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = Major;
            hash = hash * 7 + Minor;
            hash = hash * 7 + Build;
            hash = hash * 7 + Revision;
            return Revision;
        }

        /// <summary>
        /// Equals operator.
        /// </summary>
        /// <param name="v1">first object</param>
        /// <param name="v2">second object</param>
        /// <returns></returns>
        public static bool operator ==(Version v1, Version v2)
        {
            return Object.Equals(v1, v2);
        }

        /// <summary>
        /// Not equals operator.
        /// </summary>
        /// <param name="v1">first object</param>
        /// <param name="v2">second object</param>
        /// <returns></returns>
        public static bool operator !=(Version v1, Version v2)
        {
            return !Object.Equals(v1, v2);
        }

        /// <summary>
        /// Greater than operator.
        /// </summary>
        /// <param name="v1">first object</param>
        /// <param name="v2">second object</param>
        /// <returns></returns>
        public static bool operator >(Version v1, Version v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        /// <summary>
        /// Greater or equal than operator.
        /// </summary>
        /// <param name="v1">first object</param>
        /// <param name="v2">second object</param>
        /// <returns></returns>
        public static bool operator >=(Version v1, Version v2)
        {
            return v1.CompareTo(v2) >= 0;
        }

        /// <summary>
        /// Less than operator.
        /// </summary>
        /// <param name="v1">first object</param>
        /// <param name="v2">second object</param>
        /// <returns></returns>
        public static bool operator <(Version v1, Version v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        /// <summary>
        /// Less or equal than operator.
        /// </summary>
        /// <param name="v1">first object</param>
        /// <param name="v2">second object</param>
        /// <returns></returns>
        public static bool operator <=(Version v1, Version v2)
        {
            return v1.CompareTo(v2) <= 0;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(Major);

            if (Minor >= 0)
            {
                builder.Append('.');
                builder.Append(Minor);

                if (Build >= 0)
                {
                    builder.Append('.');
                    builder.Append(Build);

                    if (Revision > 0)
                    {
                        builder.Append('.');
                        builder.Append(Revision);
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="fieldCount">Field count</param>
        /// <returns></returns>
        public string ToString(int fieldCount)
        {
            StringBuilder builder = new StringBuilder();

            if (fieldCount > 0)
            {
                builder.Append(Major);

                if (Minor >= 0 && fieldCount > 1)
                {
                    builder.Append('.');
                    builder.Append(Minor);

                    if (Build >= 0 && fieldCount > 2)
                    {
                        builder.Append('.');
                        builder.Append(Build);

                        if (Revision > 0 && fieldCount > 3)
                        {
                            builder.Append('.');
                            builder.Append(Revision);
                        }
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Parses a string
        /// </summary>
        /// <param name="value"></param>
        private static void Parse(string value, out int major, out int minor, out int build, out int revision)
        {
            // Sanity check
            if (value == null)
            {
                throw new ArgumentNullException("version");
            }

            // Split into fields
            string[] fields = value.Split('.');

            major = 0;
            minor = -1;
            build = -1;
            revision = -1;

            if (fields.Length > 4)
            {
                throw new ArgumentException("Invalid version string.");
            }
            if (fields.Length > 3)
            {
                revision = int.Parse(fields[3], CultureInfo.InvariantCulture);
            }
            if (fields.Length > 2)
            {
                build = int.Parse(fields[2], CultureInfo.InvariantCulture);
            }
            if (fields.Length > 1)
            {
                minor = int.Parse(fields[1], CultureInfo.InvariantCulture);
            }
            if (fields.Length > 0)
            {
                major = int.Parse(fields[0], CultureInfo.InvariantCulture);
            }
            else
            {
                throw new ArgumentException("Invalid version string.");
            }
        }
    }
}