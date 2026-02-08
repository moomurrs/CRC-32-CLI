using CRC_32_CLI;

namespace TestSuiteCRC32 {
    public class UnitTest1 {
        [Fact]
        public void TextTest() {
            string lorem = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam vitae ex sed nulla luctus suscipit quis quis nisl. Mauris sit amet tellus eu eros hendrerit interdum. Fusce sagittis vitae dui sed vestibulum. Curabitur volutpat convallis nisl, vitae pellentesque dolor interdum ut. Vestibulum et lectus vel leo accumsan fermentum. Donec purus lorem, laoreet a scelerisque sit amet, semper ut mi. Donec blandit urna eget magna dictum, eget rhoncus justo tincidunt. In et sapien et augue tincidunt lacinia. Nam ut ultrices sem. Vestibulum mauris eros, semper vitae felis et, euismod ullamcorper tortor. Morbi ut bibendum neque. Duis sem tellus, tempor ut nisl ac, varius luctus mi. Duis sit amet erat ullamcorper justo elementum venenatis. Integer efficitur nisl vel congue rhoncus. Aliquam ut justo sed orci vulputate aliquet. Maecenas id suscipit erat. Nulla eu metus vestibulum lectus accumsan malesuada. Aenean et tincidunt mauris. Suspendisse egestas molestie urna a condimentum. Mauris laoreet fringilla finibus. Ut eu nulla iaculis, egestas dui in, mollis augue. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Aliquam dui nunc, sagittis vel lacus quis, pulvinar porttitor enim. Mauris eget risus ut velit ullamcorper ultricies. Aenean id massa gravida, dictum quam et, sollicitudin neque. Mauris ut semper ex. Maecenas at ullamcorper neque. Donec lacinia nulla ut est luctus, ut congue massa rhoncus. Vivamus varius est et efficitur vehicula. Nunc congue mauris in sapien vestibulum, non semper est fringilla. Donec interdum magna vel semper finibus. Fusce dictum felis eros, vitae vehicula purus finibus porta. Etiam ipsum lectus, tincidunt a cursus eu, fringilla sit amet ipsum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce et pharetra risus. Ut tincidunt id mauris non condimentum. Mauris ultricies ante sed nisl molestie consectetur sit amet at lectus. Vestibulum cursus posuere feugiat. Suspendisse faucibus ex sed diam tristique rutrum nec et dui. Donec quis eros tristique, auctor mi sit amet, gravida eros. Cras eu condimentum nunc. Proin quis sem id felis semper imperdiet eget accumsan tellus. Praesent ut semper quam. Cras mattis ac elit sed euismod. Aenean pellentesque elit et dolor fermentum sollicitudin. Aenean hendrerit nibh augue, ac finibus sem dictum eu. Aliquam dui felis, varius et dictum sit amet, eleifend nec tellus. Donec vehicula tempor mi, mollis venenatis erat ullamcorper sed. Nulla iaculis metus ut blandit eleifend. Mauris iaculis sapien ac imperdiet viverra. Interdum et malesuada fames ac ante ipsum primis in faucibus. Donec placerat feugiat dolor quis aliquam. Ut molestie cursus velit, et cursus eros imperdiet a. Integer congue massa vitae dolor pharetra semper. Interdum et malesuada fames ac ante ipsum primis in faucibus. Pellentesque vulputate consectetur ante et cursus. Nullam lorem diam, placerat non lectus sit amet, rutrum auctor eros. Morbi congue diam ac convallis hendrerit. Curabitur at lectus tellus. Vestibulum at cursus nunc, sit amet congue ligula. Cras egestas finibus est in convallis. Vestibulum sit amet lacinia leo. Pellentesque vel vulputate nisi, a commodo ex. In posuere euismod erat, eget posuere purus malesuada eget. Pellentesque id odio sed orci pulvinar facilisis.";

            // 0x154CC658 - reverse divisor
            // command: CRC-32 -t "..."
            string[] command1 = { "-t", lorem };
            string ans1 = CRC32.Run(command1, false);

            Assert.Equal("0x154CC658", ans1);

            // 0x81C245B0 - forward divisor
            // command: CRC-32 -t -f "..."
            string[] command2 = { "-t", lorem, "-f" };
            string ans2 = CRC32.Run(command2, false);
            Assert.Equal("0x81C245B0", ans2);
        }

        [Fact]
        public void TextFailureTest() {
            string text = "ABC";
            string[] command = { "-t" };
            Assert.Equal("", CRC32.Run(command, false));

            command = [];
            Assert.Equal("", CRC32.Run(command, false));

            command = [""];
            Assert.Equal("", CRC32.Run(command, false));

            command = ["-t", text, "-a"];
            Assert.Equal("", CRC32.Run(command, false));

            command = ["a", "-t", text];
            Assert.Equal("", CRC32.Run(command, false));

            command = ["-t", "-f", text];
            Assert.Equal("", CRC32.Run(command, false));
        }

        [Fact]
        public void FileTest() {
            string fileName = "46_Olympus.wav";

            // reverse divisor
            string[] command = { fileName };
            Assert.Equal("0x9BB5E842", CRC32.Run(command, false));

            command = [fileName, "-d"];
            Assert.Equal("2612389954", CRC32.Run(command, false));

            // forward divisor
            command = [fileName, "-f"];
            Assert.Equal("0x2954987B", CRC32.Run(command, false));

            command = [fileName, "-d", "-f"];
            Assert.Equal("693409915", CRC32.Run(command, false));

        }

        [Fact]
        public void FileTestFailure() {
            string fileName = "dummy.wav";

            // reverse divisor
            string[] command = { fileName };
            Assert.Equal("", CRC32.Run(command, false));

            command = [fileName, "-d"];
            Assert.Equal("", CRC32.Run(command, false));

            // forward divisor
            command = [fileName, "-f"];
            Assert.Equal("", CRC32.Run(command, false));

            command = [fileName, "-d", "-f"];
            Assert.Equal("", CRC32.Run(command, false));

        }
    }

}