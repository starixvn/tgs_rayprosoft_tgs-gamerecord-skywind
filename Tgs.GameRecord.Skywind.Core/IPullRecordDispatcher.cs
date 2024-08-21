using Tgs.GameRecord.Skywind.Core.ApplicationServices;

namespace Tgs.GameRecord.Skywind.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
