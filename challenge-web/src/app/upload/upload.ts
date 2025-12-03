import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-upload',
  imports: [CommonModule],
  templateUrl: './upload.html',
  styleUrl: './upload.css',
})
export class Upload implements OnInit {
  presignedUrl: string | null = null;
  isLoading: boolean = false;
  isUploading: boolean = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  selectedFile: File | null = null;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchPresignedUrl();
  }

  async fetchPresignedUrl(): Promise<void> {
    this.isLoading = true;
    this.errorMessage = null;
    
    try {
      const response = await firstValueFrom(
        this.http.get<any>('http://localhost:9090/api/import?isNew=true', {
          responseType: 'json'
        })
      );
      
      // Handle different response formats
      if (typeof response === 'string') {
        this.presignedUrl = response;
      } else if (response && typeof response === 'object') {
        // Check common property names for URL
        this.presignedUrl = response.url || response.presignedUrl || response.uploadUrl || response.data?.url;
        
        if (!this.presignedUrl) {
          throw new Error('Presigned URL not found in response');
        }
      } else {
        throw new Error('Invalid response format');
      }
    } catch (error: any) {
      this.errorMessage = error?.message || 'Failed to fetch presigned URL';
      console.error('Error fetching presigned URL:', error);
    } finally {
      this.isLoading = false;
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.errorMessage = null;
      this.successMessage = null;
    }
  }

  async uploadFile(): Promise<void> {
    if (!this.selectedFile) {
      this.errorMessage = 'Please select a file first';
      return;
    }

    if (!this.presignedUrl) {
      this.errorMessage = 'Presigned URL not available. Please try refreshing the page.';
      return;
    }

    this.isUploading = true;
    this.errorMessage = null;
    this.successMessage = null;

    try {
      // Upload file to presigned URL using PUT method (common for presigned URLs)
      const headers = new HttpHeaders({
        'Content-Type': this.selectedFile.type || 'application/octet-stream',
      });

      const fileContent = await this.readFileAsArrayBuffer(this.selectedFile);
      
      await firstValueFrom(
        this.http.put(this.presignedUrl, fileContent, { headers })
      );

      this.successMessage = `File "${this.selectedFile.name}" uploaded successfully!`;
      this.selectedFile = null;
      
      // Reset file input
      const fileInput = document.getElementById('file-input') as HTMLInputElement;
      if (fileInput) {
        fileInput.value = '';
      }
    } catch (error: any) {
      this.errorMessage = error?.message || 'Failed to upload file';
      console.error('Error uploading file:', error);
    } finally {
      this.isUploading = false;
    }
  }

  private readFileAsArrayBuffer(file: File): Promise<ArrayBuffer> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result as ArrayBuffer);
      reader.onerror = reject;
      reader.readAsArrayBuffer(file);
    });
  }

  onRetry(): void {
    this.fetchPresignedUrl();
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }
}

