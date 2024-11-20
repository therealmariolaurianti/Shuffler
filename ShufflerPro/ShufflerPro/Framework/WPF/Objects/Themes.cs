using ShufflerPro.Client.Entities;

namespace ShufflerPro.Framework.WPF.Objects;

public static class Themes
{
    public static IEnumerable<Theme> Items => new[]
    {
        new Theme("DFD6ECEC-AE89-4781-8DF6-A60FE5F995F4", "Red"),
        new Theme("C181E55B-8B2F-4AFE-99C5-AFA2135A37B7", "Green"),
        new Theme("7685B3F4-358D-4E28-A4F8-8716564F90FD", "Blue"),
        new Theme("BB049F86-8ABD-4853-AECD-2A1F9D90BA7C", "Purple"),
        new Theme("13C32C54-DD7F-48C1-841E-A31E2BCBFE82", "Orange"),
        new Theme("8690521C-1467-4628-937E-F0389377312F", "Lime"),
        new Theme("F10AA33A-AB7E-43F5-B310-6AC94E9D331E", "Emerald"),
        new Theme("5179711F-7E1B-423A-B671-055AF28F185D", "Teal"),
        new Theme("B700E374-6105-4CAA-BFA6-AC64E69AE3A8", "Cyan"),
        new Theme("E7842097-B76C-47CE-B1F5-84434E815DF8", "Cobalt"),
        new Theme("2B5F00A1-86AF-40E4-80F1-45C10D3F5941", "Indigo"),
        new Theme("411AA490-28CA-43C9-9D17-8865C22AE6BD", "Violet"),
        new Theme("43DA8675-8C48-452D-AF17-1E2BBC7633C8", "Pink"),
        new Theme("90793893-6ACC-44F0-ACC0-52C9B9E44B47", "Magenta"),
        new Theme("1520BC92-2317-4D3A-BCFD-7DB9184398D6", "Crimson"),
        new Theme("DA8523B4-E89E-4BE2-A88B-17782DD6FCCB", "Amber"),
        new Theme("BB27B945-7143-424B-BE47-841BCDF4AEB9", "Yellow"),
        new Theme("08BE3219-B529-416D-8D75-ECB57525659F", "Brown"),
        new Theme("E3825EA6-33B5-49C3-8233-FD6BF47FB71F", "Olive"),
        new Theme("21E41DA6-3ED7-483C-BA84-2F645283F784", "Steel"),
        new Theme("FF2C18A8-E307-4409-BC94-4D129DB1E986", "Mauve"),
        new Theme("7989A738-BAB3-42B2-8A05-59AC7F32C5CC", "Taupe"),
        new Theme("D12E57A5-E533-4F88-A8EC-622B102BDA94", "Sienna")
    }.OrderBy(d => d.Name);
}