import { useTheme } from "next-themes"
import { Toaster as Sonner, toast as sonnerToast, type ExternalToast } from "sonner"

type ToastType = 'success' | 'error' | 'info' | 'warning'

export interface ToastMethodOptions {
  duration?: number
  action?: {
    label: string
    onClick: () => void
  }
}

const toastStyles = {
  success: "bg-green-50 border-green-200 text-green-800 dark:bg-green-950 dark:border-green-800 dark:text-green-200",
  error: "bg-red-50 border-red-200 text-red-800 dark:bg-red-950 dark:border-red-800 dark:text-red-200", 
  info: "bg-blue-50 border-blue-200 text-blue-800 dark:bg-blue-950 dark:border-blue-800 dark:text-blue-200",
  warning: "bg-yellow-50 border-yellow-200 text-yellow-800 dark:bg-yellow-950 dark:border-yellow-800 dark:text-yellow-200"
}

function getToastOptions(type: ToastType, options: ToastMethodOptions = {}): ExternalToast {
  const { duration = 4000, action } = options

  return {
    duration,
    className: `border shadow-lg ${toastStyles[type]}`,
    action: action
      ? {
          label: action.label,
          onClick: action.onClick,
        }
      : undefined,
  }
}

type ToastApi = {
  success: (message: string, options?: ToastMethodOptions) => ReturnType<typeof sonnerToast.success>
  error: (message: string, options?: ToastMethodOptions) => ReturnType<typeof sonnerToast.error>
  info: (message: string, options?: ToastMethodOptions) => ReturnType<typeof sonnerToast.info>
  warning: (message: string, options?: ToastMethodOptions) => ReturnType<typeof sonnerToast.warning>
}

export const toast: ToastApi = {
  success: (message, options) => sonnerToast.success(message, getToastOptions('success', options)),
  error: (message, options) => sonnerToast.error(message, getToastOptions('error', options)),
  info: (message, options) => sonnerToast.info(message, getToastOptions('info', options)),
  warning: (message, options) => sonnerToast.warning(message, getToastOptions('warning', options)),
}

export function Toaster() {
  const { theme = "system" } = useTheme()

  return (
    <Sonner
      theme={theme as "light" | "dark" | "system"}
      position="top-right"
      richColors
      closeButton
    />
  )
} 