using System;

namespace NetMQ.Core.Transports.Pgm
{
    // typedef struct _RM_SENDER_STATS {
    //    ULONGLONG DataBytesSent;
    //    ULONGLONG TotalBytesSent;
    //    ULONGLONG NaksReceived;
    //    ULONGLONG NaksReceivedTooLate;
    //    ULONGLONG NumOutstandingNaks;
    //    ULONGLONG NumNaksAfterRData;
    //    ULONGLONG RepairPacketsSent;
    //    ULONGLONG BufferSpaceAvailable;
    //    ULONGLONG TrailingEdgeSeqId;
    //    ULONGLONG LeadingEdgeSeqId;
    //    ULONGLONG RateKBitsPerSecOverall;
    //    ULONGLONG RateKBitsPerSecLast;
    //    ULONGLONG TotalODataPacketsSent;
    //} RM_SENDER_STATS;

    /// <summary>
    /// Modeled after RM_SENDER_STATS
    /// </summary>
    public struct PgmSenderStats
    {
        internal const int BufferLength = 8 * 13; // 8 bytes / public ulong * 13 ulongs

        /// <summary>
        ///# client data bytes sent out so far
        ///</summary>
        public ulong DataBytesSent { get; private set; }

        /// <summary>
        ///SPM, OData and RData bytes
        ///</summary>
        public ulong TotalBytesSent { get; private set; }

        /// <summary>
        ///# NAKs received so far
        ///</summary>
        public ulong NaksReceived { get; private set; }

        /// <summary>
        ///# NAKs recvd after window advanced
        ///</summary>
        public ulong NaksReceivedTooLate { get; private set; }

        /// <summary>
        ///# NAKs yet to be responded to
        ///</summary>
        public ulong NumOutstandingNaks { get; private set; }

        /// <summary>
        ///# NAKs yet to be responded to
        ///</summary>
        public ulong NumNaksAfterRData { get; private set; }

        /// <summary>
        ///# Repairs (RDATA) sent so far
        ///</summary>
        public ulong RepairPacketsSent { get; private set; }

        /// <summary>
        ///# partial messages dropped
        ///</summary>
        public ulong BufferSpaceAvailable { get; private set; }

        /// <summary>
        ///smallest (oldest) Sequence Id in the window
        ///</summary>
        public ulong TrailingEdgeSeqId { get; private set; }

        /// <summary>
        ///largest (newest) Sequence Id in the window
        ///</summary>
        public ulong LeadingEdgeSeqId { get; private set; }

        /// <summary>
        ///Internally calculated send-rate from the beginning
        ///</summary>
        public ulong RateKBitsPerSecOverall { get; private set; }

        /// <summary>
        ///Send-rate calculated every INTERNAL_RATE_CALCULATION_FREQUENCY
        ///</summary>
        public ulong RateKBitsPerSecLast { get; private set; }

        /// <summary>
        ///# ODATA packets sent so far
        ///</summary>
        public ulong TotalODataPacketsSent { get; private set; }

        internal PgmSenderStats(byte[] buffer)
        {
            if (buffer.Length != BufferLength)
                throw new ArgumentException(nameof(buffer));

            int startIndex = 0;

            DataBytesSent = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            TotalBytesSent = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            NaksReceived = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            NaksReceivedTooLate = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            NumOutstandingNaks = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            NumNaksAfterRData = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            RepairPacketsSent = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            BufferSpaceAvailable = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            TrailingEdgeSeqId = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            LeadingEdgeSeqId = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            RateKBitsPerSecOverall = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            RateKBitsPerSecLast = BitConverter.ToUInt64(buffer, startIndex);
            startIndex += 8;
            TotalODataPacketsSent = BitConverter.ToUInt64(buffer, startIndex);
        }

        /// <summary>
        /// ToString implementation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{nameof(PgmSenderStats)} {{ {nameof(DataBytesSent)} = {DataBytesSent}, {nameof(TotalBytesSent)} = {TotalBytesSent}, {nameof(NaksReceived)} = {NaksReceived}, {nameof(NaksReceivedTooLate)} = {NaksReceivedTooLate}, {nameof(NumOutstandingNaks)} = {NumOutstandingNaks}, {nameof(NumNaksAfterRData)} = {NumNaksAfterRData}, {nameof(RepairPacketsSent)} = {RepairPacketsSent}, {nameof(BufferSpaceAvailable)} = {BufferSpaceAvailable}, {nameof(TrailingEdgeSeqId)} = {TrailingEdgeSeqId}, {nameof(LeadingEdgeSeqId)} = {LeadingEdgeSeqId}, {nameof(RateKBitsPerSecOverall)} = {RateKBitsPerSecOverall}, {nameof(RateKBitsPerSecLast)} = {RateKBitsPerSecLast}, {nameof(TotalODataPacketsSent)} = {TotalODataPacketsSent} }}";
    }
}