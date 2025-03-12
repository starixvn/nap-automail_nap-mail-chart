using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Client.Models;
using System.Collections.Generic;

namespace Fanex.Chart.Business.Services;

public interface MessageService
{
    bool Send(List<string> files);
}

public class MessageServiceImp(LetstalkClientService letstalkService) : MessageService
{
    public bool Send(List<string> files)
    {
        var messageSubject = string.Empty;
        var messageContent = string.Empty;
        var letstalkAttachments = CreateAttachments(files);
        var letstalkModel = CreateLetstalkMessageModel(messageSubject, messageContent, letstalkAttachments);
        var result = letstalkService.SendMessage(letstalkModel);
        return result;
    }

    private static LetstalkModel CreateLetstalkMessageModel(string subject, string content, List<LetstalkAttachment> letstalkAttachments)
    {
        return new LetstalkModel
        {
            TargetName = AppConfigService.AppConfig.TargetName,
            Subject = subject,
            Content = content,
            Group = AppConfigService.AppConfig.Group,
            LetstalkAttachments = letstalkAttachments ?? []
        };
    }

    private static List<LetstalkAttachment> CreateAttachments(List<string> files)
    {
        var attachments = new List<LetstalkAttachment>();
        files.ForEach(file =>
        {
            attachments.Add(new LetstalkAttachment
            {
                FileName = file,
                Bytes = System.IO.File.ReadAllBytes(file)
            });
        });

        return attachments;
    }
}