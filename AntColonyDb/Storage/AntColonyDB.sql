﻿ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [AntColonyDB], FILENAME = 'C:\Users\user\AntColonyDB.mdf', SIZE = 8192 KB, FILEGROWTH = 65536 KB) TO FILEGROUP [PRIMARY];

