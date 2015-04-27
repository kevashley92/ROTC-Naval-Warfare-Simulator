public class LocalizedItem
{

	public string file;
	public string label;
    public string append;

    public LocalizedItem (string file, string label)
    {
        this.file = file;
        this.label = label;
        append = "";
    }

	public LocalizedItem (string file, string label, string append)
	{
		this.file = file;
		this.label = label;
        this.append = append;
	}

	public string ToString ()
	{
		return label + append;
	}

}
