﻿ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [AntColonyDB_log], FILENAME = 'C:\Users\user\AntColonyDB_log.ldf', SIZE = 8192 KB, MAXSIZE = 2097152 MB, FILEGROWTH = 65536 KB);

