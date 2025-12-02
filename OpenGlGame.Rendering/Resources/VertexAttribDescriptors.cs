using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace OpenGlGame.Rendering.Resources;

public sealed class VertexAttribDescriptors : ResourceBase
{
    public class AttribDescriptor
    {
        /**
         * The location of this descriptor.
         */
        public int Location { get; set; }

        /**
         * The number of slots for this location used. Must be between 0 and 4.
         */
        public int NumSlotsUsed { get; set; }

        /**
         * The data type of the primitive values, usually GL_FLOAT.
         */
        public VertexAttribPointerType PrimitiveDataType { get; set; }

        /**
         * The size in bytes from the attribute of this instance to the same attribute of the next instance
         */
        public int Stride { get; set; }

        /**
         * The offset (in bytes) in the data buffer.
         */
        public int Offset { get; set; }

        public AttribDescriptor()
        {
        }

        public AttribDescriptor(
            int location,
            int numSlotsUsed,
            VertexAttribPointerType primitiveDataType,
            int stride,
            int offset)
        {
            Location = location;
            NumSlotsUsed = numSlotsUsed;
            PrimitiveDataType = primitiveDataType;
            Stride = stride;
            Offset = offset;
        }
    }


    public List<AttribDescriptor> Attributes { get; }

    internal VertexAttribDescriptors(uint resourceId, IList<AttribDescriptor> attributes) : base(resourceId)
    {
        Attributes = new List<AttribDescriptor>(attributes);
    }

    internal VertexAttribDescriptors(uint resourceId, params AttribDescriptor[] attributes) : base(resourceId)
    {
        Attributes = new List<AttribDescriptor>(attributes);
    }
}