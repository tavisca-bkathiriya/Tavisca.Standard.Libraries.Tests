using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tavisca.Libraries.Lock;
using Tavisca.Platform.Common.LockManagement;
using Xunit;

namespace Tavisca.Libraries.LockManagement.Tests
{
    public class GlobalLockTest
    {
        [Fact]
        public async Task Global_read_Lock_Should_Aquire_At_First_Time()
        {
            Mock<ILockProvider> lockProvider = new Mock<ILockProvider>();
            lockProvider.Setup(x => x.TryGetLockAsync(It.IsAny<string>(), It.IsAny<LockType>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            lockProvider.Setup(x => x.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<LockType>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var globalLockProvider = new GlobalLock(lockProvider.Object);
            using (var GlobalLock = await globalLockProvider.EnterReadLock("test"))
            {

            }

            lockProvider.Verify(x => x.TryGetLockAsync(It.IsAny<string>(), It.IsAny<LockType>(), It.IsAny<CancellationToken>()),Times.Once);
        }
    }
}
