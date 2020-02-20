using System;

using Disboard.Misskey.Enums;

namespace Disboard.Misskey.Extensions
{
    public static class PermissionExtensions
    {
        public static string ToStr(this Permission permission)
        {
            switch (permission)
            {
                case Permission.AccountRead:
                    return "read:account";

                case Permission.AccountWrite:
                    return "write:account";

                case Permission.BlocksRead:
                    return "read:blocks";

                case Permission.BlocksWrite:
                    return "write:blocks";

                case Permission.DriveRead:
                    return "read:drive";

                case Permission.DriveWrite:
                    return "write:drive";

                case Permission.FavoritesRead:
                    return "read:favorites";

                case Permission.FavoriteWrite:
                    return "write:favorites";

                case Permission.FollowingWrite:
                    return "write:following";

                case Permission.FollowingRead:
                    return "read:following";

                case Permission.MessagingRead:
                    return "read:messaging";

                case Permission.MessagingWrite:
                    return "write:messaging";

                case Permission.MutesRead:
                    return "read:mutes";

                case Permission.MutesWrite:
                    return "write:mutes";

                case Permission.NoteWrite:
                    return "write:notes";

                case Permission.NotificationsRead:
                    return "read:notifications";

                case Permission.NotificationWrite:
                    return "write:notifications";

                case Permission.ReactionsRead:
                    return "read:reactions";

                case Permission.ReactionWrite:
                    return "write:reactions";

                case Permission.VoteWrite:
                    return "write:vote";

                case Permission.AccountRead2:
                    return "account/read";

                case Permission.AccountWrite2:
                    return "account/write";

                default:
                    throw new ArgumentOutOfRangeException(nameof(permission), permission, null);
            }
        }
    }
}