// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace FlatSharpTests.Oracle
{

using global::System;
using global::FlatBuffers;

public struct FiveByteStructTable : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static FiveByteStructTable GetRootAsFiveByteStructTable(ByteBuffer _bb) { return GetRootAsFiveByteStructTable(_bb, new FiveByteStructTable()); }
  public static FiveByteStructTable GetRootAsFiveByteStructTable(ByteBuffer _bb, FiveByteStructTable obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public FiveByteStructTable __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public FiveByteStruct? Vector(int j) { int o = __p.__offset(4); return o != 0 ? (FiveByteStruct?)(new FiveByteStruct()).__assign(__p.__vector(o) + j * 8, __p.bb) : null; }
  public int VectorLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<FiveByteStructTable> CreateFiveByteStructTable(FlatBufferBuilder builder,
      VectorOffset VectorOffset = default(VectorOffset)) {
    builder.StartObject(1);
    FiveByteStructTable.AddVector(builder, VectorOffset);
    return FiveByteStructTable.EndFiveByteStructTable(builder);
  }

  public static void StartFiveByteStructTable(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddVector(FlatBufferBuilder builder, VectorOffset VectorOffset) { builder.AddOffset(0, VectorOffset.Value, 0); }
  public static void StartVectorVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(8, numElems, 4); }
  public static Offset<FiveByteStructTable> EndFiveByteStructTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FiveByteStructTable>(o);
  }
};


}
