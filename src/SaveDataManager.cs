using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Windows;
using System.Xml;

namespace GoalTracker
{
    // Any I/O or serialization exceptions encountered during file handling will show a popup (MessageBox)
    // with information about the error (todo: separate that out of this class?)
    class SaveDataManager
    {
        // private static readonly string AppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        // private static readonly string DefaultSaveDirectory =
        //     (AppDataDirectory == string.Empty) ? string.Empty : Path.Combine(AppDataDirectory, "GoalTracker");

        // Use the executable directory instead of AppData to make the application more portable
        // (todo: save user.config in the .exe directory too)
        private static readonly string DefaultSaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");


        private DataContractSerializer serializer = new DataContractSerializer(typeof(UserData));

        public SaveDataManager()
        {
            if (Properties.Settings.Default.IsFirstLaunch)
            {
                Properties.Settings.Default.IsFirstLaunch = false;
                Properties.Settings.Default.Save();
                SetAndCreateDefaultSaveDirectory();
            }
        }

        /// <summary>
        /// The path to save to and load from when another path isn't specified.
        /// </summary>
        public string SaveFilePath
        {
            get => Path.Combine(
                Properties.Settings.Default.SaveDirectory,
                Properties.Settings.Default.SaveFileName
                );
        }

        /// <summary>
        /// Saves the given UserData to the given path.
        /// </summary>
        /// <returns>True if serialization succeeded; false if it failed,
        /// due to either an error serializing the data or an I/O error.</returns>
        public bool SerializeData(string filePath, UserData userData)
        {
            try
            {
                using (FileStream file = File.Create(filePath))
                {
                    serializer.WriteObject(file, userData);
                }
            }
            catch (Exception ex) when (IsFileException(ex) || IsSerializationException(ex))
            {
                AlertException(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Loads UserData from the given file, or creates a new, empty save file if the file doesn't exist.
        /// If successful, <see cref="SaveFilePath"/> is updated to the new path. 
        /// </summary>
        /// <remarks>
        /// Creating a new file will fail if the path includes a directory that doesn't exist.
        /// </remarks>
        /// <returns>The loaded or created UserData, or null if either a) the file exists and can't be read,
        /// or b) the file does not exist and can't be created.</returns>
        public UserData LoadOrCreateSaveData(string filePath)
        {
            UserData userData;
            bool fileExists = DeserializeData(SaveFilePath, out userData);

            if (!fileExists)
            {
                userData = CreateNewUserData(filePath);
            }

            if (userData != null && filePath != SaveFilePath)
            {
                SetSavePath(filePath);
            }

            return userData;
        }

        /// <summary>
        /// Creates a new save file at the specified location.
        /// If successful, <see cref="SaveFilePath"/> is updated to the new path. 
        /// </summary>
        /// <returns>UserData representing the new, empty save data,
        /// or null if unable to create the file at the specified location</returns>
        public UserData CreateSaveData(string filePath)
        {
            UserData userData = CreateNewUserData(filePath);
            if (userData != null)
            {
                SetSavePath(filePath);
            }
            return userData;
        }

        /// <summary>
        /// Loads a save file from the specified location.
        /// If successful, <see cref="SaveFilePath"/> is updated to the new path. 
        /// </summary>
        /// <returns>The loaded UserData, or null if unable to load the data.</returns>
        public UserData LoadSaveData(string filePath)
        {
            DeserializeData(filePath, out UserData userData);
            if (userData != null)
            {
                SetSavePath(filePath);
            }
            return userData;
        }

        /// <summary>
        /// Moves the current save file to the specified location.
        /// If successful, <see cref="SaveFilePath"/> is updated to the new path. 
        /// </summary>
        /// <remarks>
        /// Note: if a file already exists at the given path, it will be overwritten.
        /// </remarks>
        public void MoveSaveData(string newFilePath)
        {
            if (newFilePath == SaveFilePath)
            {
                return;
            }

            try
            {
                if (File.Exists(newFilePath))
                {
                    File.Delete(newFilePath);
                }
                File.Move(SaveFilePath, newFilePath);
            }
            catch (Exception ex) when (IsFileException(ex))
            {
                AlertException(ex);
                return;
            }

            SetSavePath(newFilePath);
        }


        /// <summary>
        /// Attempts to load UserData from the given file path.
        /// </summary>
        /// <param name="loadedData">Contains the loaded UserData, or null if the file can't be opened or parsed.</param>
        /// <returns>False if the file does not exist but could be created; else true.</returns>
        private bool DeserializeData(string filePath, out UserData loadedData)
        {
            UserData userData = null;
            
            try
            {
                using (FileStream file = File.OpenRead(filePath))
                {
                    userData = (UserData)serializer.ReadObject(file);
                }
            }
            catch (FileNotFoundException)
            {
                loadedData = null;
                return false;
            }
            catch (Exception ex) when (IsFileException(ex))
            {
                AlertException(ex);
            }
            catch (Exception ex) when (IsDeserializationException(ex))
            {
                AlertDeserializationException(ex);
            }

            loadedData = userData;
            return true;
        }

        /// <summary>
        /// Creates a new, empty save file at the given path.
        /// </summary>
        /// <returns>Null if failed to create the file.</returns>
        private UserData CreateNewUserData(string path)
        {
            UserData userData = new UserData();
            
            bool success = SerializeData(path, userData);

            if (!success)
            {
                Console.WriteLine($"Failed to create save file at {path}");
                return null;
            }
            else
            {
                Console.WriteLine($"Created save file at {path}");
                return userData;
            }
        }

        /// <summary>
        /// Sets the save directory to <see cref="DefaultSaveDirectory"/>,
        /// creating the directory if it doesn't already exist.
        /// </summary>
        private void SetAndCreateDefaultSaveDirectory()
        {
            string path = DefaultSaveDirectory;

            Properties.Settings.Default.SaveDirectory = path;
            Properties.Settings.Default.Save();
            Console.WriteLine($"Set save file directory to {path}");

            if (path != string.Empty && !Directory.Exists(path))
            {
                CreateSaveDirectory(path);
            }
        }

        /// <returns>False if unable to create the directory.</returns>
        private bool CreateSaveDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex) when (IsFileException(ex))
            {
                AlertException(ex);
                return false;
            }
            Console.WriteLine($"Created directory {path}");
            return true;
        }

        // newPath should be a full path (directory and filename)
        private void SetSavePath(string newPath)
        {
            // (assumes newPath is always supplied from a FileDialog that won't return an invalid path)
            Properties.Settings.Default.SaveDirectory = Path.GetDirectoryName(newPath);
            Properties.Settings.Default.SaveFileName = Path.GetFileName(newPath);
            Properties.Settings.Default.Save();
            Console.WriteLine($"Set save file path to {SaveFilePath}");
        }

        // Exceptions that result in failure to open or write a file should usually be handled the same way
        // (by alerting the user and canceling the operation), so SaveDataManager handles these "file errors" as a group.
        // 
        // (Most possible file name/permissions errors should be caught by the FileDialog that supplies the file path,
        // but might as well be ready to handle them here too just in case.)
        private bool IsFileException(Exception ex)
        {
            return ex is UnauthorizedAccessException
                || ex is NotSupportedException
                || ex is ArgumentNullException
                || ex is ArgumentException
                || ex is IOException;  // includes DirectoryNotFoundException, PathTooLongException
        }

        // Only exceptions that can (theoretically) occur during normal program operation are handled;
        // serialization errors that result from the UserData object being badly formatted
        // indicate a bug in the program and should be thrown.
        private bool IsSerializationException(Exception ex)
        {
            return ex is QuotaExceededException;
        }

        private bool IsDeserializationException(Exception ex)
        {
            // The exceptions DataContractSerializer.ReadObject() can throw aren't well-documented (that I can find),
            // so this list may be incomplete
            return ex is XmlException
                || ex is SerializationException;
        }

        private void AlertException(Exception e)
        {
            MessageBox.Show($"{e.GetType()}: {e.Message}", "File error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Show a probably more helpful message when a file can't be parsed.
        private void AlertDeserializationException(Exception e)
        {
            string message = "Unable to read the save file; it may be corrupted or in the wrong format.\n\n"
                + $"(Exception: {e.GetType()} - {e.Message})";
            MessageBox.Show(message, "Failed to read file", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
