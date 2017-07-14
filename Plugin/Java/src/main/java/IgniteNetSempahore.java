import org.apache.ignite.IgniteCheckedException;
import org.apache.ignite.IgniteSemaphore;
import org.apache.ignite.internal.binary.BinaryRawReaderEx;
import org.apache.ignite.internal.binary.BinaryRawWriterEx;
import org.apache.ignite.internal.processors.platform.PlatformAsyncResult;
import org.apache.ignite.internal.processors.platform.PlatformTarget;
import org.apache.ignite.internal.processors.platform.memory.PlatformMemory;
import org.jetbrains.annotations.Nullable;

public class IgniteNetSempahore implements PlatformTarget {
    private final IgniteSemaphore semaphore;

    public IgniteNetSempahore(IgniteSemaphore semaphore) {
        this.semaphore = semaphore;
    }

    public long processInLongOutLong(int i, long l) throws IgniteCheckedException {
        return 0;
    }

    public long processInStreamOutLong(int i, BinaryRawReaderEx binaryRawReaderEx) throws IgniteCheckedException {
        return 0;
    }

    public long processInStreamOutLong(int i, BinaryRawReaderEx binaryRawReaderEx, PlatformMemory platformMemory) throws IgniteCheckedException {
        return 0;
    }

    public void processInStreamOutStream(int i, BinaryRawReaderEx binaryRawReaderEx, BinaryRawWriterEx binaryRawWriterEx) throws IgniteCheckedException {

    }

    public PlatformTarget processInStreamOutObject(int i, BinaryRawReaderEx binaryRawReaderEx) throws IgniteCheckedException {
        return null;
    }

    public PlatformTarget processInObjectStreamOutObjectStream(int i, @Nullable PlatformTarget platformTarget, BinaryRawReaderEx binaryRawReaderEx, BinaryRawWriterEx binaryRawWriterEx) throws IgniteCheckedException {
        return null;
    }

    public void processOutStream(int i, BinaryRawWriterEx binaryRawWriterEx) throws IgniteCheckedException {

    }

    public PlatformTarget processOutObject(int i) throws IgniteCheckedException {
        return null;
    }

    public PlatformAsyncResult processInStreamAsync(int i, BinaryRawReaderEx binaryRawReaderEx) throws IgniteCheckedException {
        return null;
    }

    public Exception convertException(Exception e) {
        return null;
    }
}
