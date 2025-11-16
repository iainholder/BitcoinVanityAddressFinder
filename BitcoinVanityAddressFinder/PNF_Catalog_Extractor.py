import customtkinter as ctk
import tkinter as tk
from tkinter import filedialog, messagebox
import os
import fitz  # PyMuPDF
import re
from PIL import Image, ImageTk
import io

class PDFExtractorApp:
    def __init__(self, root):
        self.root = root
        self.root.title("PNF Catalog Extractor")
        self.root.geometry("800x600")

        # Create tabview
        self.tabview = ctk.CTkTabview(self.root, width=780, height=580)
        self.tabview.pack(pady=10, padx=10, fill="both", expand=True)

        # Create tabs
        self.tabview.add("Main")
        self.tabview.add("Preview")

        # Main tab content
        self.main_tab = self.tabview.tab("Main")
        self.setup_main_tab()

        # Preview tab content
        self.preview_tab = self.tabview.tab("Preview")
        self.setup_preview_tab()

        # Variables
        self.pdf_var = tk.StringVar()
        self.filter_var = tk.StringVar(value="All")
        self.custom_pattern_var = tk.StringVar()
        self.output_dir_var = tk.StringVar()

    def setup_main_tab(self):
        # PDF Selection
        pdf_label = ctk.CTkLabel(self.main_tab, text="Select PDF File:")
        pdf_label.pack(pady=5)
        pdf_entry = ctk.CTkEntry(self.main_tab, textvariable=self.pdf_var, width=400)
        pdf_entry.pack(pady=5)
        pdf_button = ctk.CTkButton(self.main_tab, text="Browse", command=self.browse_pdf)
        pdf_button.pack(pady=5)

        # Filter Options
        filter_label = ctk.CTkLabel(self.main_tab, text="Filter Options:")
        filter_label.pack(pady=5)
        filter_options = ["All", "PNF", "Custom"]
        filter_menu = ctk.CTkOptionMenu(self.main_tab, variable=self.filter_var, values=filter_options, command=self.on_filter_change)
        filter_menu.pack(pady=5)

        # Custom Pattern Entry (initially hidden)
        self.custom_pattern_label = ctk.CTkLabel(self.main_tab, text="Custom Pattern:")
        self.custom_pattern_entry = ctk.CTkEntry(self.main_tab, textvariable=self.custom_pattern_var, width=400)

        # Output Directory
        output_label = ctk.CTkLabel(self.main_tab, text="Output Directory:")
        output_label.pack(pady=5)
        output_entry = ctk.CTkEntry(self.main_tab, textvariable=self.output_dir_var, width=400)
        output_entry.pack(pady=5)
        output_button = ctk.CTkButton(self.main_tab, text="Browse", command=self.browse_output_dir)
        output_button.pack(pady=5)

        # Buttons
        extract_button = ctk.CTkButton(self.main_tab, text="Extract Pages", command=self.extract_pages)
        extract_button.pack(pady=10)
        preview_button = ctk.CTkButton(self.main_tab, text="Preview Filtered PDF", command=self.preview_pdf)
        preview_button.pack(pady=5)

    def setup_preview_tab(self):
        # This will be populated when preview is clicked
        self.preview_label = ctk.CTkLabel(self.preview_tab, text="Click 'Preview Filtered PDF' to load preview here.")
        self.preview_label.pack(pady=20)

    def browse_pdf(self):
        file_path = filedialog.askopenfilename(filetypes=[("PDF files", "*.pdf")])
        if file_path:
            self.pdf_var.set(file_path)

    def browse_output_dir(self):
        dir_path = filedialog.askdirectory()
        if dir_path:
            self.output_dir_var.set(dir_path)

    def on_filter_change(self, value):
        if value == "Custom":
            self.custom_pattern_label.pack(pady=5)
            self.custom_pattern_entry.pack(pady=5)
        else:
            self.custom_pattern_label.pack_forget()
            self.custom_pattern_entry.pack_forget()

    def extract_pages(self):
        # Implementation for extract_pages
        pass

    def preview_pdf(self):
        # Get the selected PDF file
        selected_pdf = self.pdf_var.get()
        if not selected_pdf:
            messagebox.showerror("Error", "Please select a PDF file to preview.")
            return

        # Check if PDF exists
        if not os.path.exists(selected_pdf):
            messagebox.showerror("Error", "Selected PDF file does not exist.")
            return

        # Switch to preview tab
        self.tabview.set("Preview")

        # Clear previous content
        for widget in self.preview_tab.winfo_children():
            widget.destroy()

        # Create scrollable frame for preview
        scrollable_frame = ctk.CTkScrollableFrame(self.preview_tab, fg_color="#dbc078")
        scrollable_frame.pack(fill="both", expand=True, padx=10, pady=10)

        # Add title
        title_label = ctk.CTkLabel(scrollable_frame, text="Filtered PDF Preview", font=("Arial", 16, "bold"), text_color="#000000")
        title_label.pack(pady=10)

        # Add PDF info
        pdf_info_label = ctk.CTkLabel(scrollable_frame, text=f"PDF: {os.path.basename(selected_pdf)}", font=("Arial", 12), text_color="#000000")
        pdf_info_label.pack(pady=5)

        # Add filter info
        filter_info = f"Filter: {self.filter_var.get()}"
        if self.filter_var.get() == "Custom":
            filter_info += f" - Custom Pattern: {self.custom_pattern_var.get()}"
        filter_info_label = ctk.CTkLabel(scrollable_frame, text=filter_info, font=("Arial", 12), text_color="#000000")
        filter_info_label.pack(pady=5)

        # Add preview content placeholder
        preview_label = ctk.CTkLabel(scrollable_frame, text="PDF preview content would be displayed here.\n\nThis is a placeholder for the actual PDF rendering.", font=("Arial", 12), text_color="#000000", justify="left")
        preview_label.pack(pady=10, fill="both", expand=True)

        # Add back to main button
        back_button = ctk.CTkButton(scrollable_frame, text="Back to Main", command=lambda: self.tabview.set("Main"))
        back_button.pack(pady=10)

if __name__ == "__main__":
    ctk.set_appearance_mode("light")
    ctk.set_default_color_theme("blue")
    root = ctk.CTk()
    app = PDFExtractorApp(root)
    root.mainloop()