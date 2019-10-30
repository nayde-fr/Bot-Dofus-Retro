using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bot_Dofus_1._29._1.Common.Network;

/*
    Este archivo es parte del proyecto BotDofus_1.29.1
    BotDofus_1.29.1 Copyright (C) 2019 Alvaro Prendes — Todos los derechos reservados.
    Creado por Alvaro Prendes
    web: http://www.salesprendes.com
*/

namespace Bot_Dofus_1._29._1.Common.Frames.Transport
{
    public static class PacketReceiver
    {
        public static readonly List<PacketHandler> handlers = new List<PacketHandler>();

        public static void Initialize()
        {
            Assembly asm = typeof(Frame).GetTypeInfo().Assembly;

            foreach (MethodInfo methodInfo in asm.GetTypes().SelectMany(x => x.GetMethods()).Where(m => m.GetCustomAttributes(typeof(PacketAttribute), false).Length > 0))
            {
                PacketAttribute packetAttribute = methodInfo.GetCustomAttributes(typeof(PacketAttribute), true)[0] as PacketAttribute;
                Type handlerType = Type.GetType(methodInfo.DeclaringType.FullName);

                object instance = Activator.CreateInstance(handlerType, null);
                handlers.Add(new PacketHandler(instance, packetAttribute.Identifier, methodInfo));
            }
        }

        public static void Receive(TcpClient client, string rawPacketData)
        {
            PacketHandler handler = handlers.Find(m => rawPacketData.StartsWith(m.PacketIdentifier));

            if (handler != null)
                handler.Information.Invoke(handler.Instance, new object[2] { client, rawPacketData });
        }
    }
}